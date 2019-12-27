#include "animation.h"

Animation::~Animation() {
	if (name) delete[] name;
	if (frames) delete[] frames;
}

int Animation::get_next_frame_index(int frame_index) {
	if (frame_count == 0) return 0;
	if (frame_index < 0) return 0;
	if (frame_index < get_last_frame_index()) return frame_index + 1;
	return 0;
}

Frame* Animation::get_next_frame(Frame* frame) {
	if (frame_count == 0) return nullptr;
	return get_frame(get_next_frame_index(frame->index));
}

int Animation::get_last_frame_index() {
	if (frame_count == 0) return 0;
	return frame_count - 1;
}

Frame* Animation::get_frame(int frame_index) {
	if (frame_count == 0) return nullptr;
	if (frame_index < 0) return nullptr;
	if (frame_index > get_last_frame_index()) return nullptr;
	return frames + frame_index;
}

void Animation::load(FILE* stream, int name_length, int frame_count) {
	if (name) delete[] name;
	name = new char[name_length + 1];
	this->frame_count = frame_count;
	if (frames) delete[] frames;
	frames = new Frame[frame_count];
	char buffer[ANIMATION_DATA_FILE_PARSING_BUFFER];
	int next_frame_to_process_index = 0;

	/* name %s */
	const char* str_name = "name";
	/* frame_start */
	const char* str_frame_start = "frame_start";
	/* animation_end */
	const char* str_animation_end = "animation_end";

	while (fgets(buffer, sizeof(buffer), stream)) {
		if (strncmp(buffer, str_name, sizeof(str_name))) {
			char* name_format_scan_string = new char[8 + Globals::digit_count(name_length) + 1];
			sprintf(name_format_scan_string, "name %%%ds", name_length);
			sscanf(buffer, "name %s", name);
			delete name_format_scan_string;
		}
		else if (strncmp(buffer, str_frame_start, sizeof(str_frame_start)))
			frames[next_frame_to_process_index++].load(stream);
		else if (strncmp(buffer, str_animation_end, sizeof(str_animation_end))) return;
	}
}
