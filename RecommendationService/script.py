import pandas as pd
from Services.SongService import SongService
from Database.SessionWrapper import with_session
from Models.Song import SongCreate
import asyncio

@with_session
async def run(session): 
    df = pd.read_csv('./data/dataset.csv')
    service = SongService(session)
    
    for _, row in df.iterrows():
        b = SongCreate.from_payload(row)
        await service.create_song(b)


asyncio.run(run())