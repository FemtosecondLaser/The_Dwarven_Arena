#include "time_controller.h"

Time_Controller::Time_Controller() {
	delta_time = 0;
	last_updated = 0;
	time_state = TIME_STATE_PLAY;
}

void Time_Controller::update_time() {
	Uint32 ticks = SDL_GetTicks();
	if (time_state == TIME_STATE_PAUSE) delta_time = 0;
	else delta_time = (ticks - last_updated) / 1000.0;
	last_updated = ticks;
}

void Time_Controller::pause() {
	time_state = TIME_STATE_PAUSE;
}

void Time_Controller::resume() {
	time_state = TIME_STATE_PLAY;
}

void Time_Controller::reset() {
	delta_time = 0;
	last_updated = SDL_GetTicks();
}

Time_Controller Time_Controller::time_controller;