from sqlalchemy import Column, Integer, Float, Enum, TIMESTAMP
from sqlalchemy.sql import func
from Database.base import Base
import enum

class InteractionTypeEnum(str, enum.Enum):
    play = "play"
    like = "like"
    dislike = "dislike"

class UserSongInteraction(Base):
    __tablename__ = "user_song_interactions"

    id = Column(Integer, primary_key=True)
    user_id = Column(Integer)
    song_id = Column(Integer)
    interaction_type = Column(Enum(InteractionTypeEnum), nullable=False)
    timestamp = Column(TIMESTAMP(timezone=True), server_default=func.now(), nullable=False)
    weight = Column(Float, nullable=True)
