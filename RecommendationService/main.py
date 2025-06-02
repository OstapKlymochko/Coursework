from contextlib import asynccontextmanager
from fastapi import FastAPI, APIRouter, Response
from Services.UserInteractionService import UserInteractionService
from Database.SessionWrapper import with_session
from Services.MusicRecommender import MusicRecommenderSingleton
from Services.SongService import SongService
from Services.Scheduler import start_scheduler

from rabbitmq_consumer import consume
import asyncio


@asynccontextmanager
async def lifespan(app: FastAPI):
    task = asyncio.create_task(consume())
    start_scheduler()
    yield
    task.cancel()


app = FastAPI()
app = FastAPI(lifespan=lifespan)
router = APIRouter(prefix="/api/recommendations")


@router.get('/hello')
async def get_hello():
    a = await get_data()
    return {'message': a}

@router.get("/{user_id}", summary="Get recommendations for a user")
async def recommend(user_id: str):
    recommender = await MusicRecommenderSingleton.get_instance()
    recommendations = recommender.recommend_user(user_id, top_n=10)
    songs = await get_songs(ids=recommendations)
    print(len(songs))
    return {"user_id": user_id, "recommended_song_ids": recommendations, 'songs': songs}

app.include_router(router)


@with_session
async def get_data(session):
    try:
        return await UserInteractionService(session).get_interactions()
    except Exception as e:
        print(e)

@with_session
async def get_songs(session, ids):
    try:
        return await SongService(session).get_by_ids_list(map(int, ids))
    except Exception as e:
        print(e)
