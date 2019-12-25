#include "globals.h"

int Globals::screen_width = 800, Globals::screen_height = 600, Globals::screen_scale = 1;

SDL_Renderer* Globals::renderer = nullptr;

int Globals::digit_count(int i) {
	int n = 1;
	if (i < 0) i = (i == INT_MIN) ? INT_MAX : -i;
	while (i > 9) {
		i /= 10;
		n++;
	}
	return n;
}

char* Globals::get_resource_path(char* resource_name) {
	char* resource_path = new char[strlen(RESOURCES_DIR_RELATIVE_PATH) + strlen(resource_name) + 1];
	strcpy(resource_path, RESOURCES_DIR_RELATIVE_PATH);
	strcat(resource_path, resource_name);
	return resource_path;
}
