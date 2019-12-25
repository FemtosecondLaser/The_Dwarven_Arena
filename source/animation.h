#ifndef ANIMATION_H
#define ANIMATION_H

#include "frame.h"
#include "globals.h"

struct Animation {
	char* name;
	Frame* frames;
	int frame_count;

	~Animation();

	int get_next_frame_index(int frame_index);
	Frame* get_next_frame(Frame* frame);
	int get_last_frame_index();
	Frame* get_frame(int frame_index);
	void load(FILE* stream, int name_length, int frame_count);
};

#endif