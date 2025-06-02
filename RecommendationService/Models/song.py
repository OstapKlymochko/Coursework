from pydantic import BaseModel


class SongCreate(BaseModel):
    title: str
    danceability: float
    energy: float
    key: int
    loudness: float
    tempo: float
    time_signature: int

    @classmethod
    def from_payload(cls, payload: dict):
        return cls(
            title=payload["title"],
            danceability=payload["danceability"],
            energy=payload["energy"],
            key=payload["key"],
            loudness=payload["loudness"],
            tempo=payload["tempo"],
            time_signature=payload["time_signature"]
        )


class SongRead(SongCreate):
    id: int