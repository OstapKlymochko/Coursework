from pydantic import BaseModel
from typing import Optional
from datetime import datetime
from enum import Enum


class UserSongInteractionTypeEnum(str, Enum):
    play = "play"
    like = "like"
    dislike = "dislike"


class UserSongInteractionCreate(BaseModel):
    user_id: int
    song_id: int
    interaction_type: UserSongInteractionTypeEnum
    weight: Optional[float] = None

    @classmethod
    def from_payload(cls, payload: dict):
        interaction_type = UserSongInteractionTypeEnum(
            payload["interactionType"])

        weights = {
            UserSongInteractionTypeEnum.like: 3,
            UserSongInteractionTypeEnum.dislike: -2,
            UserSongInteractionTypeEnum.play: payload.get("listenTime", 1),
        }

        weight = weights[interaction_type]

        return cls(
            user_id=payload["userId"],
            song_id=payload["songId"],
            interaction_type=interaction_type,
            weight=weight
        )


# class UserSongInteraction(UserSongInteractionCreate):
#     timestamp: datetime

#     def __init__(self, queue_payload: dict):
#         super.__init__(queue_payload)
#         self.timestamp = queue_payload['timestamp']
