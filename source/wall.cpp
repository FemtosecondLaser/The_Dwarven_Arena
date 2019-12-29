#include "wall.h"

const char* Wall::a_idle = "idle";

Wall::Wall(Animation_Set* animation_set) {
	label = L_WALL;
	this->animation_set = animation_set;
	can_be_passed_through = 0;
	collision_box_width = 32;
	collision_box_height = 32;
	collision_box_vertical_offset = -16;
	update_collision_box();
	animate(STATE_IDLING, 1);
}

void Wall::update() {
	update_collision_box();
	if (current_frame && current_animation) {
		frame_timer += Time_Controller::time_controller.delta_time;
		if (frame_timer >= current_frame->duration) {
			current_frame = current_animation->get_next_frame(current_frame);
			frame_timer = 0;
		}
	}
}

void Wall::animate(Entity_State state, int force_first_frame) {
	current_animation = animation_set->get_animation(a_idle);
	current_frame = current_animation->get_frame(0);
}

void Wall::update_collisions() {
	// NOOP
}