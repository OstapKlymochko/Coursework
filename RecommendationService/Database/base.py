from typing import Generic, TypeVar, Type, Optional, List
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy.future import select
from pydantic import BaseModel
from sqlalchemy.orm import DeclarativeMeta, declarative_base

ModelType = TypeVar("ModelType", bound=DeclarativeMeta)
CreateSchemaType = TypeVar("CreateSchemaType", bound=BaseModel)

Base = declarative_base()


class BaseService(Generic[ModelType, CreateSchemaType]):
    def __init__(self, model: Type[ModelType], db: AsyncSession):
        self.model = model
        self.db = db

    async def get(self, **filters) -> Optional[ModelType]:
        stmt = select(self.model).filter_by(**filters)
        result = await self.db.execute(stmt)
        return result.scalars().first()

    async def get_all(self, **filters) -> List[ModelType]:
        stmt = select(self.model).filter_by(**filters)
        result = await self.db.execute(stmt)
        return result.scalars().all()

    async def get_all_2(self, ids: list = None) -> List[ModelType]:
        stmt = select(self.model)

        if ids is not None:
            # Припускаємо, що у твоєї моделі є колонка id
            stmt = stmt.filter(self.model.id.in_(ids))
            
        result = await self.db.execute(stmt)
        return result.scalars().all()

    async def create(self, obj_in: CreateSchemaType) -> ModelType:
        obj = self.model(**obj_in.dict())
        self.db.add(obj)
        await self.db.commit()
        await self.db.refresh(obj)
        return obj

    async def update(self, filters: dict, updates: dict) -> Optional[ModelType]:
        obj = await self.get(**filters)
        if not obj:
            return None
        for key, value in updates.items():
            setattr(obj, key, value)
        await self.db.commit()
        await self.db.refresh(obj)
        return obj

    async def delete(self, **filters) -> bool:
        obj = await self.get(**filters)
        if obj:
            await self.db.delete(obj)
            await self.db.commit()
            return True
        return False
