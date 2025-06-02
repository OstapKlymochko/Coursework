from Database.Models.UserSongInteraction import UserSongInteraction
from Database.base import BaseService
from sqlalchemy.ext.asyncio import AsyncSession
import pandas as pd

from Models.UserSongInteractionModel import UserSongInteractionCreate, UserSongInteractionTypeEnum


class UserInteractionService:
    db_service: BaseService

    def __init__(self, db: AsyncSession):
        self.db_service = BaseService(UserSongInteraction, db)

    async def get_interactions(self):
        return await self.db_service.get_all()

    async def create_interaction(self, interaction: UserSongInteractionCreate):
        return await self.db_service.create(interaction)

    async def get_user_interactions(self, user_id, filters={}):
        return await self.db_service.get_all(**filters, user_id=user_id)

    async def delete_interaction(self, user_id, song_id, type):
        if type not in [UserSongInteractionTypeEnum.like, UserSongInteractionTypeEnum.dislike]:
            return
        return await self.db_service.delete(user_id=user_id, song_id=song_id, interaction_type=type)

    async def toggle_interaction(self, user_id, song_id, type, weight):
        prev_value = 'like' if type == 'dislike' else 'dislike'
        await self.db_service.update({'user_id': user_id, 'song_id': song_id, 'interaction_type': prev_value}, {'interaction_type': type, 'weight': weight})

    async def get_all_as_dataframe(self):
        records = await self.db_service.get_all()
        data = [r.__dict__ for r in records]
        for d in data:
            d['userId'] = str(d.pop('user_id'))
            d['songId'] = str(d.pop('song_id'))
        return pd.DataFrame(data)
