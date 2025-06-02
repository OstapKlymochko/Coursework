from Database.base import BaseService
from sqlalchemy.ext.asyncio import AsyncSession
from Database.Models.Song import Song
import pandas as pd

class SongService:
    db_service: BaseService

    def __init__(self, db: AsyncSession):
        self.db_service = BaseService(Song, db)

    async def create_song(self, song):
        return await self.db_service.create(song)
    
    async def get_all_as_dataframe(self):
        records = await self.db_service.get_all()
        data = [r.__dict__ for r in records]
        for d in data:
            d['id'] = str(d['id'])
        return pd.DataFrame(data)

    async def get_song_by_id(self, id):
        return await self.db_service.get({'id': id})

    async def get_by_ids_list(self, ids):
        return await self.db_service.get_all_2(ids)
