import yaml
import json
import sys

if __name__ == '__main__':
    if len(sys.argv) > 0:
        yamlFile = open(sys.argv[1], 'r')
        yamlString = yamlFile.read()
        yamlFile.close
        yamlObj = yaml.load(yamlString)
        json.dump(yamlObj, sys.stdout)
        
        raise(SystemExit)