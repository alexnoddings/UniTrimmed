from typing import Dict, List
from core.tool import Tool
from glob import glob
from core.aggregator import aggregate
from openpyxl import load_workbook


class XlsxReader(Tool):
    def __init__(self, args: Dict[str, str], working_dir: str = ""):
        super().__init__(args, working_dir)
        self.in_files = args["in"]

    def run(self, data: List[Dict]) -> List[Dict]:
        temp = []
        for file_name in glob(self.in_files):
            workbook = load_workbook(file_name)
            rows_iterator = iter(workbook.active.rows)
            headers = next(rows_iterator)
            file_data = []
            for row in rows_iterator:
                school = {}
                for column in range(0, len(row)):
                    key = headers[column].value
                    value = row[column].value
                    school[key] = value
                # Ignore schools with invalid or missing URNs
                if "URN" in school.keys() and school["URN"] is not None:
                    file_data.append(school)
            temp.append(file_data)
        return aggregate(temp)


entry = XlsxReader
