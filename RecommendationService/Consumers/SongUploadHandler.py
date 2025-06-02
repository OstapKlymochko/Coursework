from .BaseConsumer import MessageHandler
# from Services.AudioFeatureExtractor import AudioFeatureExtractor
from Services.EssentiaExtractor import EssentiaExtractor
from Services.S3Service import S3Service
from Services.SongService import SongService

from Models.Song import SongCreate
from Database.SessionWrapper import with_session


class SongUploadHandler(MessageHandler):
    message_type = "Common.Contracts:SongUploadedContract"

    def __init__(self):
        super().__init__()

    @with_session
    async def handle(self, payload: dict, session):

        feature_extractor = EssentiaExtractor()
        url = S3Service().get_presigned_url(payload['songFileKey'])
        data = await feature_extractor.analyze_url(url)
        payload = {**payload, **data}
        print('payload', payload)

        service = SongService(session)
        
        song_create = SongCreate.from_payload(payload)
        # await service.create_song(song_create)

MessageHandler.register(SongUploadHandler)
