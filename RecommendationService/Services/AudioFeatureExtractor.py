import os
import aiohttp
import librosa
import numpy as np
import tempfile
import subprocess
import asyncio

from config import FFMPEG_PATH

class AudioFeatureExtractor:
    async def analyze_url(self, url: str) -> dict:
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as resp:
                if resp.status != 200:
                    raise Exception(f"Failed to download audio, status {resp.status}")
                
                with tempfile.NamedTemporaryFile(delete=False, suffix=".mp3") as tmp_mp3:
                    tmp_mp3.write(await resp.read())
                    mp3_path = tmp_mp3.name

        wav_path = mp3_path.replace(".mp3", ".wav")


        # Запускаємо перетворення в окремому потоці
        await asyncio.to_thread(self._convert_to_wav, mp3_path, wav_path)

        # Аналіз також в окремому потоці
        features = await asyncio.to_thread(self._analyze_audio, wav_path)

        # Чистимо тимчасові файли
        try:
            os.remove(mp3_path)
            os.remove(wav_path)
        except Exception:
            pass

        return features

    def _convert_to_wav(self, input_path: str, output_path: str):
        subprocess.run([
            FFMPEG_PATH, "-y", "-i", input_path,
            "-ac", "1", "-ar", "22050", output_path
        ], check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)

    def _analyze_audio(self, file_path: str) -> dict:
        y, sr = librosa.load(file_path, sr=None, mono=True)

        # Loudness (RMS to dB)
        rms = librosa.feature.rms(y=y)[0]
        loudness = float(np.mean(librosa.amplitude_to_db(rms)))

        # Energy
        energy = float(np.sum(y ** 2) / len(y))

        # Tempo
        onset_env = librosa.onset.onset_strength(y=y, sr=sr)
        tempo = float(librosa.beat.tempo(onset_envelope=onset_env, sr=sr)[0])

        # Danceability proxy
        tempogram = librosa.feature.tempogram(onset_envelope=onset_env, sr=sr)
        danceability = float(np.mean(tempogram))

        # Key
        chroma = librosa.feature.chroma_cqt(y=y, sr=sr)
        key_index = int(np.argmax(np.mean(chroma, axis=1)))

        # Time signature approximation
        _, beat_frames = librosa.beat.beat_track(y=y, sr=sr)
        ts = 4 if len(beat_frames) < 2 else int(np.clip(np.median(np.diff(beat_frames)), 3, 7))

        return {
            "danceability": round(danceability, 4),
            "energy": round(energy, 4),
            "key": key_index,
            "loudness": round(loudness, 4),
            "tempo": round(tempo, 2),
            "time_signature": ts
        }
