OBJS = source/*.cpp
COMPILER = g++
INCLUDE_PATHS = -I./include/SDL2
LIBRARY_PATHS = -L./lib
COMPILER_FLAGS = -w -Wl,-subsystem,windows
LINKER_FLAGS = -lmingw32 -lSDL2main -lSDL2 -lSDL2_image -lSDL2_ttf
OUTPUT_DIR = output
OUTPUT_FILE = Dwarven_Arena

build : $(OBJS)
	if exist $(OUTPUT_DIR)\ ( echo $(OUTPUT_DIR) exists ) ELSE ( md $(OUTPUT_DIR) && echo $(OUTPUT_DIR) created)
	$(COMPILER) $(OBJS) $(INCLUDE_PATHS) $(LIBRARY_PATHS) $(COMPILER_FLAGS) $(LINKER_FLAGS) -o $(OUTPUT_DIR)/$(OUTPUT_FILE)
	xcopy dependencies $(OUTPUT_DIR) /y /e
	
clean :
	if exist $(OUTPUT_DIR)\ ( rd /s /q $(OUTPUT_DIR) ) ELSE ( echo $(OUTPUT_DIR) does not exist)