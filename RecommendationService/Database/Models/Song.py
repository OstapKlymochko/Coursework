from sqlalchemy import Column, Integer, Float, String
from Database.base import Base  # Або звідти, де в тебе Base


class Song(Base):
    __tablename__ = "songs"

    id = Column(Integer, primary_key=True, index=True)
    title = Column(String, nullable=False)

    danceability = Column(Float, nullable=False)
    energy = Column(Float, nullable=False)
    key = Column(Integer, nullable=False)
    loudness = Column(Float, nullable=False)
    tempo = Column(Float, nullable=False)
    time_signature = Column(Integer, nullable=False)
