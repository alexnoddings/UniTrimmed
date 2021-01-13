from typing import Dict, List
from core.tool import Tool


class Stripper(Tool):
    def __init__(self, args: Dict[str, str], working_dir: str = ""):
        super().__init__(args, working_dir)
        self.strict = args["strict"].lower() == "true" if "strict" in args.keys() else False
        self.keys = args["keys"]

    def run(self, data: List[Dict]) -> List[Dict]:
        data = [{k: v for k, v in d.items() if k in self.keys} for d in data]
        if self.strict:
            data = [d for d in data if all([k in d.keys() for k in self.keys])]
            data = [d for d in data if all([v is not None and v != "" for v in d.values()])]
        return data


entry = Stripper
