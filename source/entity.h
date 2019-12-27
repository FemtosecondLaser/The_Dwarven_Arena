#ifndef ENTITY_H
#define ENTITY_H

#include <math.h>
#include "animation_set.h"
#include "time_controller.h"

// labels
#define L_PROTAGONIST "protagonist"
#define L_ENEMY "enemy"

struct Entity {
	enum Entity_Direction {
		DIRECTION_UP,
		DIRECTION_DOWN,
		DIRECTION_LEFT,
		DIRECTION_RIGHT
	};
	enum Entity_State {
		STATE_IDLING,
		STATE_WALKING,
		STATE_DASHING,
		STATE_ATTACKING,
		STATE_BEING_DEAD
	};

	static Entity** entities;
	static int entity_count;
	char* label;
	Entity_State state;
	float x, y;
	int is_moving;
	Entity_Direction direction;
	float angle;
	float move_speed;
	float move_speed_upper_limit;
	float move_lerp = 4;
	float slide_angle;
	float slide_amount;
	int can_be_passed_through;
	int can_collide_with_non_pass_through_entities;
	int is_active;
	SDL_Rect current_collision_box;
	SDL_Rect last_collision_box;
	SDL_Rect union_current_last_collision_box;
	int collision_box_width, collision_box_height;
	float collision_box_vertical_offset;
	Animation_Set* animation_set;
	Animation* current_animation;
	Frame* current_frame;
	float frame_timer;

	virtual void update();
	virtual void render();
	virtual void move(float angle);
	virtual void update_movement();
	virtual void update_collision_box();
	virtual void animate(Entity_State state, int force_first_frame) = 0;
	virtual void update_collisions();
	static float distance_between_rectangles(SDL_Rect* first, SDL_Rect* second);
	static float distance_between_entities(Entity* first, Entity* second);
	static float angle_between_entities(Entity* first, Entity* second);
	static int is_colliding(SDL_Rect* first, SDL_Rect* second);
	static Entity_Direction angle_to_direction(float angle);
	static float angle_between_points(float first_x, float first_y, float second_x, float second_y);
	static float angle_between_rectangles(SDL_Rect* first, SDL_Rect* second);
	static int compare_entities(Entity* first, Entity* second);
	static void remove_inactive_entities(Entity** entities, int entity_count, int delete_inactive);
	static void remove_entities(Entity** entities, int entity_count, int delete_inactive);
};

#endif