import os
import subprocess
import re

dirname = os.path.dirname(os.path.abspath(__file__))

start_time_map = {}

files = [f for f in os.listdir('.') if os.path.isfile(f)]
for f in files:
	if (f.endswith(".mkv") or f.endswith(".mka")):
		fullfilename = os.path.join(dirname, f)
		#print(fullfilename)
		cmnd = ['ffprobe', '-show_entries', 'format=start_time', '-loglevel', 'quiet', fullfilename]
		p = subprocess.Popen(cmnd, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
		out, err =  p.communicate()
		outstring = str(out)
		
		#print (outstring)
		found = re.findall("\d+\.\d+", outstring)
		for time in found:
			start_time_map[fullfilename] = time
		
print (start_time_map)