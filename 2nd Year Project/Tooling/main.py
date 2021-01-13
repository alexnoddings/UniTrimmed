from core.process import load as load_process
import yaml
from sys import argv

processes = []

with open("cfg/processes.yaml") as file:
    data = yaml.load(file.read())

for process_data in data["processes"]:
    process_id = process_data["id"]
    tools_data = process_data["tools"]
    process = load_process(process_id, tools_data)
    processes.append(process)

if __name__ == "__main__":
    target_process_id = " ".join(argv[1:])
    target_process = None
    for process in processes:
        if process.id == target_process_id:
            target_process = process
            break
    if target_process is None:
        print(f'No process with id {target_process_id}. Valid processes are: [{", ".join([p.id for p in processes])}]')
    else:
        target_process.run()
