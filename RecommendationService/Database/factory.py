from fastapi import Depends
from sqlalchemy.ext.asyncio import AsyncSession
from .database import get_db
from .base import BaseService
from typing import Type

def get_service(model: Type):
    def _get_service(db: AsyncSession = Depends(get_db)):
        return BaseService(model, db)
    return _get_service
