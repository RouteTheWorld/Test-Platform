import json
from pprint import pprint

file = open('feedback.srt', 'w+')

def get_time_range(time):
	# Display the subtitle for 5 seconds
	time_plus5 = time + 5000
	
	hours, remainder = divmod(time, 60*60*1000)
	minutes, remainder = divmod(remainder, 60*1000)
	seconds, milliseconds = divmod(remainder, 1000)
	
	hours_plus5, remainder_plus5 = divmod(time_plus5, 60*60*1000)
	minutes_plus5, remainder_plus5 = divmod(remainder_plus5, 60*1000)
	seconds_plus5, milliseconds_plus5 = divmod(remainder_plus5, 1000)
	
	
	return "%02d:%02d:%02d,%03d --> %02d:%02d:%02d,%03d" % (hours, minutes, seconds, milliseconds, hours_plus5, minutes_plus5, seconds_plus5, milliseconds_plus5)

video_start_time = 0
index = 1

with open('feedback.txt') as data_file:
	data = json.load(data_file)
	for data_entry in data:
		note = data_entry["note"]
		time = data_entry["time"]
		if video_start_time == 0:
			video_start_time = time
		
		time_since_start = time - video_start_time
		
		pprint(time_since_start)
		
		file.write(str(index) + "\n")
		file.write(get_time_range(time_since_start) + "\n")
		file.write("- " + note + "\n")
		file.write("\n")
		
		index = index + 1
	
pprint(data)
file.close()