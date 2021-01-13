from typing import Dict, List


def aggregate(data: List[List[Dict]]) -> List[Dict]:
    aggregated = {}

    for schools_data in data:
        for school_data in schools_data:
            urn = school_data["URN"]
            if urn not in aggregated.keys():
                aggregated[urn] = school_data
            else:
                aggregated[urn] = {**aggregated[urn], **school_data}

    return [d for d in aggregated.values()]
