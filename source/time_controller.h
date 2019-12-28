#ifndef TIME_CONTROLLER_H
#define TIME_CONTROLLER_H

#include <SDL.h>

struct Time_Controller {
	enum Time_State {
		TIME_STATE_PLAY,
		TIME_STATE_PAUSE
	};

	static Time_Controller time_controller;
	Time_State time_state;
	Uint32 last_updated;
	float delta_time; /* seconds since last rendered frame */

	Time_Controller();

	void update_time();
	void pause();
	void resume();
	void reset();

};

#endif