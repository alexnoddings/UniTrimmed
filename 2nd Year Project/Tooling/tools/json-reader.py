from typing import Dict, List
from core.tool import Tool
from glob import glob
from json import loads
from core.aggregator import aggregate


class JsonReader(Tool):
    def __init__(self, args: Dict[str, str], working_dir: str = ""):
        super().__init__(args, working_dir)
        self.in_files = args["in"]

    def run(self, data: List[Dict]) -> List[Dict]:
        temp = []
        for file_name in glob(self.in_files):
            with open(file_name) as file:
                temp.append(loads(file.read()))
        return aggregate(temp)


entry = JsonReader
