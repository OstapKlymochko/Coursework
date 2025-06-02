from Consumers.BaseConsumer import MessageHandler
from Services.UserInteractionService import UserInteractionService
from Models.UserSongInteractionModel import UserSongInteractionCreate
from sqlalchemy.ext.asyncio import AsyncSession
from Database.SessionWrapper import with_session

class SongInteractionHandler(MessageHandler):
    message_type = "Common.Contracts:SongInteractionContract"

    def __init__(self):
        super().__init__()

    @with_session
    async def handle(self, payload: dict, session: AsyncSession):
        interaction = UserSongInteractionCreate.from_payload(payload)
        service = UserInteractionService(session)
        
        if payload['delete']:
            await service.delete_interaction(interaction.user_id, interaction.song_id, interaction.interaction_type)
        elif payload['toggle']:
            await service.toggle_interaction(interaction.user_id, interaction.song_id, interaction.interaction_type, interaction.weight)
        
        if payload['delete'] or payload['toggle']:
            return
        
        await service.create_interaction(interaction)


MessageHandler.register(SongInteractionHandler)
