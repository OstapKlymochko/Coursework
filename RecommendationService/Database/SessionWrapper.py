from functools import wraps
from .database import async_session

def with_session(func):
    @wraps(func)
    async def wrapper(*args, **kwargs):
        async with async_session() as session:
            return await func(*args, session=session, **kwargs)
    return wrapper
