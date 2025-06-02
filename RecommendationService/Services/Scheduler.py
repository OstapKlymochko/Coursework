from apscheduler.schedulers.asyncio import AsyncIOScheduler
from apscheduler.triggers.cron import CronTrigger
from Services.MusicRecommender import MusicRecommenderSingleton

scheduler = AsyncIOScheduler()

def start_scheduler():
    scheduler.start()

    scheduler.add_job(
        update_model_job,
        CronTrigger(minute=0),
        id="model_update",
        name="Update recommendation model hourly",
        replace_existing=True
    )

async def update_model_job():
    print("[CRON] Updating recommendation model...")
    await MusicRecommenderSingleton.update_model()  
    print("[CRON] Model updated.")
