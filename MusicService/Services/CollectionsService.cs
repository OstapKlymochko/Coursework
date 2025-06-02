using Common.CommonTypes;
using Common.Errors;
using Common.Result;
using Common.Services.Interfaces;
using MusicService.Models;
using FluentValidation;
using MusicService.Services.Interfaces;

namespace MusicService.Services
{
    public class CollectionsService : ICollectionsService
    {
        private readonly ICollectionsDbService _collectionsDbService;
        private readonly IFileLinkGeneratorService _filesService;
        private readonly IValidator<CreateCollectionModel> _collectionModelValidator;

        public CollectionsService(ICollectionsDbService collectionsDbService,
            IValidator<CreateCollectionModel> collectionModelValidator, IFileLinkGeneratorService filesService)
        {
            _collectionsDbService = collectionsDbService;
            _collectionModelValidator = collectionModelValidator;
            _filesService = filesService;
        }

        public async Task<ServiceResult<BasicResponse>> CreateCollection(CreateCollectionModel collection, int userId)
        {
            var validationResult = _collectionModelValidator.Validate(collection);
            if (!validationResult.IsValid) return new ModelError(string.Join(", ", validationResult.Errors));

            var normalizedTitle = collection.Title.Trim();
            var collectionDbRecord = await _collectionsDbService.GetCollectionByNameAsync(normalizedTitle, userId);
            if (collectionDbRecord != null) return new ModelError($"You already have a playlist with title {collection.Title}");

            var collectionDbModel = new CollectionDbModel()
            {
                OwnerId = userId,
                Title = normalizedTitle,
                Type = collection.Type,
                OwnerUsername = collection.OwnerUserName
            };
            await _collectionsDbService.CreateCollectionAsync(collectionDbModel);

            return new BasicResponse("Playlist was successfully created");
        }

        public async Task<ServiceResult<CollectionsListDto>> GetUsersCollectionsAsync(int userId)
        {
            var collections = await _collectionsDbService.GetUserCollectionsAsync(userId);
            foreach (var item in collections)
            {
                var url = await _filesService.GetPreSignedUrl(item.ImageUrl);
                item.ImageUrl = url!;
            }
            return new CollectionsListDto() { Collections = collections };
        }

        public async Task<ServiceResult<BasicResponse>> RemoveCollectionAsync(int userId, int collectionId)
        {
            var collection = await _collectionsDbService.GetCollectionByIdAsync(userId, collectionId);
            if (collection == null) return new NotFoundError("Collection does not exist");

            await _collectionsDbService.DeleteCollectionAsync(collectionId, userId);
            return new BasicResponse("Collection was successfully deleted");
        }

        public async Task<ServiceResult<SongsListDto>> GetCollectionSongsAsync(int userId, int collectionId)
        {
            var collection = await _collectionsDbService.GetCollectionByIdAsync(userId, collectionId);
            if (collection == null) return new NotFoundError("Collection does not exist");

            var songs = await _collectionsDbService.GetCollectionSongsAsync(userId, collectionId);
            foreach (var song in songs)
            {
                var logoUrl = await _filesService.GetPreSignedUrl(song.LogoUrl);
                song.LogoUrl = logoUrl!;
            }

            return new SongsListDto() { Songs = songs };
        }

        public async Task<ServiceResult<BasicResponse>> AddSongToCollectionAsync(int userId, int collectionId, int songId, bool isAuthor)
        {
            var collection = await _collectionsDbService.GetCollectionByIdAsync(userId, collectionId);
            if (collection == null) return new NotFoundError("Collection does not exist");

            if (collection.Type == "Album" && !isAuthor) return new ModelError("You are not allowed to perform this operation");

            var isSongInPlaylist = await _collectionsDbService.IsSongInCollection(songId, userId, collectionId);
            if (isSongInPlaylist) return new ModelError("Song has already been added");

            await _collectionsDbService.AddSongToCollectionAsync(songId, userId, collectionId);
            return new BasicResponse("Song was successfully added");
        }

        public async Task<ServiceResult<BasicResponse>> RemoveSongFromCollectionAsync(int userId, int collectionId, int songId, bool isAuthor)
        {
            var collection = await _collectionsDbService.GetCollectionByIdAsync(userId, collectionId);
            if (collection == null) return new NotFoundError("Collection does not exist");

            if (collection.Type == "Album" && !isAuthor) return new ModelError("You are not allowed to perform this operation");

            var isSongInPlaylist = await _collectionsDbService.IsSongInCollection(songId, userId, collectionId);
            if (!isSongInPlaylist) return new NotFoundError("Song was not found");

            await _collectionsDbService.RemoveSongFromCollectionAsync(songId, userId, collectionId);
            return new BasicResponse("Song was successfully removed");
        }
    }
}
