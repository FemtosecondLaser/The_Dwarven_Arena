#ifndef TIMECONTROLLER_H
#define TIMECONTROLLER_H

#include <SDL.h>

struct Time_Controller {
	enum Time_State {
		TIME_STATE_PLAY,
		TIME_STATE_PAUSE
	};

	Time_State time_state;
	Uint32 last_updated;
	float delta_time; /* seconds since last rendered frame */

	Time_Controller();

	void update_time();
	void pause();
	void resume();
	void reset();

	static Time_Controller time_controller;
};

#endif