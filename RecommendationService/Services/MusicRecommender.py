import os
import pickle
import pandas as pd
import numpy as np
from lightfm import LightFM
from lightfm.data import Dataset
from sqlalchemy.ext.asyncio import AsyncSession

from Services.UserInteractionService import UserInteractionService
from Database.SessionWrapper import with_session
# from config import MODEL_PATH

class MusicRecommender:
    def __init__(self, interactions_df: pd.DataFrame, train=False):
        self.interactions = interactions_df.copy()
        self.model_path = os.path.join('model', 'model.pkl')

        self.model = None
        self.dataset = None
        self.user_id_map = {}
        self.item_id_map = {}

        self.interactions['userId'] = self.interactions['userId'].astype(str)
        self.interactions['songId'] = self.interactions['songId'].astype(str)

        if os.path.exists(self.model_path) and not train:
            self._load_model()
        else:
            self._train()
            self._save_model()

    def _train(self):
        self.dataset = Dataset()
        self.dataset.fit(self.interactions['userId'], self.interactions['songId'])

        interactions, _ = self.dataset.build_interactions(
            [(row['userId'], row['songId'], row['weight']) for _, row in self.interactions.iterrows()]
        )

        self.model = LightFM(loss='warp')
        self.model.fit(interactions, epochs=10, num_threads=4)

        self.user_id_map, _, self.item_id_map, _ = self.dataset.mapping()

    def _save_model(self):
        with open(self.model_path, 'wb') as f:
            pickle.dump({
                'model': self.model,
                'dataset': self.dataset,
                'user_id_map': self.user_id_map,
                'item_id_map': self.item_id_map
            }, f)

    def _load_model(self):
        with open(self.model_path, 'rb') as f:
            data = pickle.load(f)
            self.model = data['model']
            self.dataset = data['dataset']
            self.user_id_map = data['user_id_map']
            self.item_id_map = data['item_id_map']

    def recommend_user(self, user_id: str, top_n: int = 5):
        user_id = str(user_id)
        if user_id not in self.user_id_map:
            return self.popular_items(top_n)

        user_internal = self.user_id_map[user_id]
        item_ids = list(self.item_id_map.keys())
        item_internal_ids = [self.item_id_map[i] for i in item_ids]

        scores = self.model.predict(user_internal, item_internal_ids)
        top_indices = np.argsort(-scores)[:top_n]
        return [item_ids[i] for i in top_indices]

    def similar_items(self, song_id: str, top_n: int = 5):
        song_id = str(song_id)
        if song_id not in self.item_id_map:
            return []

        idx = self.item_id_map[song_id]
        emb = self.model.item_embeddings[idx]
        norms = np.linalg.norm(self.model.item_embeddings, axis=1)
        sims = np.dot(self.model.item_embeddings, emb) / (norms * np.linalg.norm(emb))

        similar_indices = np.argsort(-sims)[1:top_n + 1]
        inv_map = {v: k for k, v in self.item_id_map.items()}
        return [inv_map[i] for i in similar_indices]

    def popular_items(self, top_n: int = 5):
        top_ids = self.interactions.groupby('songId')['weight'].sum().sort_values(ascending=False).head(top_n).index
        return list(top_ids)


class MusicRecommenderSingleton:
    _instance = None

    @classmethod
    async def get_instance(cls):
        if cls._instance is None:
            cls._instance = await cls._build_model()
        return cls._instance

    @classmethod
    async def update_model(cls):
        print("[Recommender] Updating model from database interactions...")
        cls._instance = await cls._build_model()
        print("[Recommender] Model retrained.")
        return cls._instance

    @classmethod
    @with_session
    async def _build_model(cls, session):
        interactions = await UserInteractionService(session).get_interactions()
        if not interactions:
            print("[Recommender] No interactions found.")
            return None
        
        df = pd.DataFrame([{
            "userId": str(i.user_id),
            "songId": str(i.song_id),
            "weight": i.weight
        } for i in interactions])

        return MusicRecommender(df)
            