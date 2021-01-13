from typing import Dict, List
from core.tool import Tool
from glob import glob
from json import dumps


class JsonWriter(Tool):
    def __init__(self, args: Dict[str, str], working_dir: str = ""):
        super().__init__(args, working_dir)
        self.out_file = args["out"]

    def run(self, data: List[Dict]) -> List[Dict]:
        temp = [{k: str(v) for k, v in d.items()} for d in data]
        with open(self.out_file, "w") as file:
            file.write(dumps(temp, indent=4))
        return data


entry = JsonWriter
