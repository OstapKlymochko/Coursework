import pandas as pd


class PdUtil:
    def __init__(self, df: pd.DataFrame) -> None:
        self.df = df

    def append_row(self, row: pd.DataFrame, path: str):
        self.df = pd.concat([self.df, row])
        row.to_csv(path, mode='a', header=False, index=False)