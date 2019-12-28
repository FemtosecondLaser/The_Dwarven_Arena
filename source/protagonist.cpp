#include "protagonist.h"

const char* Protagonist::a_idle_up = "idle_up";
const char* Protagonist::a_idle_down = "idle_down";
const char* Protagonist::a_idle_left = "idle_left";
const char* Protagonist::a_idle_right = "idle_right";
const char* Protagonist::a_walk_up = "walk_up";
const char* Protagonist::a_walk_down = "walk_down";
const char* Protagonist::a_walk_left = "walk_left";
const char* Protagonist::a_walk_right = "walk_right";
const char* Protagonist::a_dash_up = "dash_up";
const char* Protagonist::a_dash_down = "dash_down";
const char* Protagonist::a_dash_left = "dash_left";
const char* Protagonist::a_dash_right = "dash_right";
const char* Protagonist::a_attack_up = "attack_up";
const char* Protagonist::a_attack_down = "attack_down";
const char* Protagonist::a_attack_left = "attack_left";
const char* Protagonist::a_attack_right = "attack_right";
const char* Protagonist::a_death = "death";

Protagonist::Protagonist(Animation_Set* animation_set) {
	label = L_PROTAGONIST;
	this->animation_set = animation_set;
	x = Globals::screen_width / 2;
	y = Globals::screen_height / 2;
	move_speed = 0;
	move_speed_upper_limit = 50;
	current_health = max_health = 20;
	damage = 0;
	collision_box_width = 14;
	collision_box_height = 18;
	collision_box_vertical_offset - 14;
	direction = DIRECTION_DOWN;
	animate(STATE_IDLING, 1);
	update_collision_box();
}

void Protagonist::update() {
	if (current_health <= 0 && state != STATE_BEING_DEAD) {
		animate(STATE_BEING_DEAD, 1);
		is_moving = 0;
		death();
	}
	update_collision_box();
	update_movement();
	update_collisions();
	update_hitbox();
	update_dealt_damage();
	update_animation();
	update_invincible_timer();
}

void Protagonist::attack() {
	if (current_health > 0 && (state == STATE_WALKING || state == STATE_IDLING)) {
		is_moving = false;
		frame_timer = 0;
		animate(STATE_ATTACKING, 1);
	}
}

void Protagonist::dash() {
	if (current_health > 0 && (state == STATE_WALKING || state == STATE_IDLING)) {
		is_moving = false;
		frame_timer = 0;
		slide_angle = angle;
		slide_amount = 260;
		invincible_timer = 0.1;
		animate(STATE_DASHING, 1);
	}
}

void Protagonist::death() {
	is_moving = false;
	animate(STATE_BEING_DEAD, 1);
}

void Protagonist::resurrect() {
	is_moving = false;
	slide_amount = 0;
	current_health = max_health;
	animate(STATE_IDLING, 1);
	x = Globals::screen_width / 2;
	y = Globals::screen_height / 2;
}

void Protagonist::animate(Entity_State state, int force_first_frame) {
	this->state = state;
	if (this->state == STATE_IDLING) {
		if (direction == DIRECTION_UP) current_animation = animation_set->get_animation(a_idle_up);
		else if (direction == DIRECTION_DOWN) current_animation = animation_set->get_animation(a_idle_down);
		else if (direction == DIRECTION_LEFT) current_animation = animation_set->get_animation(a_idle_left);
		else if (direction == DIRECTION_RIGHT) current_animation = animation_set->get_animation(a_idle_right);
	}
	else if (this->state == STATE_WALKING) {
		if (direction == DIRECTION_UP) current_animation = animation_set->get_animation(a_walk_up);
		else if (direction == DIRECTION_DOWN) current_animation = animation_set->get_animation(a_walk_down);
		else if (direction == DIRECTION_LEFT) current_animation = animation_set->get_animation(a_walk_left);
		else if (direction == DIRECTION_RIGHT) current_animation = animation_set->get_animation(a_walk_right);
	}
	else if (this->state == STATE_DASHING) {
		if (direction == DIRECTION_UP) current_animation = animation_set->get_animation(a_dash_up);
		else if (direction == DIRECTION_DOWN) current_animation = animation_set->get_animation(a_dash_down);
		else if (direction == DIRECTION_LEFT) current_animation = animation_set->get_animation(a_dash_left);
		else if (direction == DIRECTION_RIGHT) current_animation = animation_set->get_animation(a_dash_right);
	}
	else if (this->state == STATE_ATTACKING) {
		if (direction == DIRECTION_UP) current_animation = animation_set->get_animation(a_attack_up);
		else if (direction == DIRECTION_DOWN) current_animation = animation_set->get_animation(a_attack_down);
		else if (direction == DIRECTION_LEFT) current_animation = animation_set->get_animation(a_attack_left);
		else if (direction == DIRECTION_RIGHT) current_animation = animation_set->get_animation(a_attack_right);
	}
	else if (this->state == STATE_BEING_DEAD) {
		current_animation = animation_set->get_animation(a_death);
	}
#ifdef DEBUG
	else printf("entity: %s; unexpected state: %d\n", this->label, (int)this->state);
#endif
	if (force_first_frame) current_frame = current_animation->get_frame(0);
	else current_frame = current_animation->get_frame(current_frame->index);
}

void Protagonist::update_animation() {
	if (current_animation && current_frame) {
		if (state == STATE_WALKING && !is_moving) animate(STATE_IDLING, 1);
		frame_timer += Time_Controller::time_controller.delta_time;
		if (frame_timer >= current_frame->duration) {
			if (current_frame->index == current_animation->get_last_frame_index()) {
				if (state == STATE_ATTACKING || state == STATE_DASHING) animate(STATE_WALKING, 1);
				else if (state == STATE_BEING_DEAD && current_health > 0) animate(STATE_WALKING, 1);
				else current_frame = current_animation->get_frame(0);
			}
			else current_frame = current_animation->get_next_frame(current_frame);
			frame_timer = 0;
		}
	}
}

void Protagonist::update_dealt_damage() {
	if (is_active && current_health > 0 && invincible_timer <= 0) {
		for (int i = 0; i < entity_count; i++) {
			if ((*(entities + i))->is_active && (*(entities + i))->label == L_ENEMY) {
				Living_Entity* enemy = (Living_Entity*)(*(entities + 1));
				if (enemy->damage > 0 && is_colliding(&current_collision_box, &enemy->hitbox)) {
					current_health -= enemy->damage;
					if (current_health > 0) invincible_timer = 0.333; // still in the game
					slide_angle = angle_between_entities(enemy, this);
					slide_amount = 180;
				}
			}
		}
	}
}