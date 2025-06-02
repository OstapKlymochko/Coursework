import os
import aiohttp
import tempfile
import subprocess
import asyncio
import numpy as np

import essentia.standard as es

class EssentiaExtractor:
    notes = ["C", "C♯/D♭", "D", "D♯/E♭", "E", "F",
             "F♯/G♭", "G", "G♯/A♭", "A", "A♯/B♭", "B"]
    SAMPLE_RATE = 44100

    async def analyze_url(self, url: str) -> dict:
        async with aiohttp.ClientSession() as session:
            async with session.get(url) as resp:
                if resp.status != 200:
                    raise Exception(f"Failed to download audio, status {resp.status}")

                with tempfile.NamedTemporaryFile(delete=False, suffix=".mp3") as tmp_mp3:
                    tmp_mp3.write(await resp.read())
                    mp3_path = tmp_mp3.name

        wav_path = mp3_path.replace(".mp3", ".wav")
        await asyncio.to_thread(self._convert_to_wav, mp3_path, wav_path)
        features = await asyncio.to_thread(self._analyze_audio, wav_path)

        try:
            os.remove(mp3_path)
            os.remove(wav_path)
        except Exception:
            pass

        return features

    def _convert_to_wav(self, input_path: str, output_path: str):
        subprocess.run([
            'ffmpeg', "-y", "-i", input_path,
            "-ac", "1", "-ar", str(self.SAMPLE_RATE), output_path
        ], check=True, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)

    def _analyze_audio(self, file_path: str) -> dict:
        audio = es.MonoLoader(filename=file_path, sampleRate=self.SAMPLE_RATE)()

        # Loudness через ReplayGain (наближено до Spotify)
        replay_gain = es.ReplayGain()(audio)
        loudness_db = max(replay_gain, -60.0)  # обмежуємо мінімум

        # Energy
        energy_raw = es.Energy()(audio)
        energy_norm = np.clip(energy_raw, 0.0, 1.0)

        # Rhythm / Tempo
        rhythm_extractor = es.RhythmExtractor2013(method="multifeature")
        tempo, beats, _, _, _ = rhythm_extractor(audio)

        # Danceability (нормалізуємо на максимум 3)
        danceability_algo = es.Danceability(sampleRate=self.SAMPLE_RATE)
        danceability, _ = danceability_algo(audio)
        danceability_norm = np.clip(danceability / 3.0, 0.0, 1.0)

        # Key extraction
        key_extractor = es.KeyExtractor(sampleRate=self.SAMPLE_RATE)
        key_str, scale, strength = key_extractor(audio)
        key_index = self.notes.index(key_str) if key_str in self.notes else -1

        # Time signature – рахуємо beats (хоча це приблизно, Spotify часто просто дає 4)
        if len(beats) >= 2:
            intervals = np.diff(beats)
            median_interval = np.median(intervals)
            # Тут замість інтервалів, просто фіксуємо 4, бо time_signature з Essentia неточний
            time_signature = 4
        else:
            time_signature = 4  # дефолтне значення

        return {
            "danceability": round(float(danceability_norm), 3),
            "energy": round(float(energy_norm), 3),
            "key": key_index,
            "loudness": round(float(loudness_db), 3),
            "tempo": round(float(tempo), 2),
            "time_signature": time_signature
        }