# Initialization
TARGET = ByteZoo.Blog.C
TARGET_DIR = bin
INC_DIR1 = .
INC_DIR2 = ./Includes
OBJ_DIR = obj

# Include files
C_INC_FILES1 = $(wildcard $(INC_DIR1)/*.h)
C_INC_FILES2 = $(wildcard $(INC_DIR2)/*.h)
ASM_INC_FILES1 = $(wildcard $(INC_DIR1)/*.inc)
ASM_INC_FILES2 = $(wildcard $(INC_DIR2)/*.inc)

# C source files in current directory   
C_FILES = $(wildcard *.c)
C_OBJ_FILES_ = $(C_FILES:.c=.o)
C_OBJ_FILES = $(patsubst %, $(OBJ_DIR)/%, $(C_OBJ_FILES_))

# Assembly source files in current directory
ASM_FILES = $(wildcard *.asm)
ASM_OBJ_FILES_ = $(ASM_FILES:.asm=.o)
ASM_OBJ_FILES = $(patsubst %, $(OBJ_DIR)/%, $(ASM_OBJ_FILES_))

# Object files
OBJ_FILES = $(C_OBJ_FILES) $(ASM_OBJ_FILES)

# Compile options
COMPILE_OPTIONS = -m64 -mavx -mavx2
C_COMPILE_OPTIONS = $(COMPILE_OPTIONS) -O3 -Wall -Wextra 
ASM_COMPILE_OPTIONS = -f elf64
# Compile options (optional)
C_COMPILE_OPTIONS_DEBUG = -g
C_COMPILE_OPTIONS_LISTFILE = -Wa,-aghl=$(OBJ_DIR)/$(basename $<).lst -save-temps=obj
ASM_COMPILE_OPTIONS_DEBUG = -g
ASM_COMPILE_OPTIONS_LISTFILE = -l $(OBJ_DIR)/$(basename $<).lst -Lm -Ls

# Create object directory
MK_OBJ_DIR := $(shell mkdir -p $(OBJ_DIR))
MK_TARGET_DIR := $(shell mkdir -p $(TARGET_DIR))

# Build rules
$(TARGET_DIR)/$(TARGET): $(OBJ_FILES)
	gcc $(COMPILE_OPTIONS) $(OBJ_FILES) -o $(TARGET_DIR)/$(TARGET)

# Build rules (C)
$(OBJ_DIR)/%.o: %.c $(C_INC_FILES1) $(C_INC_FILES2)
	gcc -I$(INC_DIR1) -I$(INC_DIR2) $(C_COMPILE_OPTIONS) $(C_COMPILE_OPTIONS_DEBUG) $(C_COMPILE_OPTIONS_LISTFILE) -c -o $@ $<

# Build rules (Assembly)
$(OBJ_DIR)/%.o: %.asm $(ASM_INC_FILES1) $(ASM_INC_FILES2)
	nasm -I$(INC_DIR1) -I$(INC_DIR2) $(ASM_COMPILE_OPTIONS) $(ASM_COMPILE_OPTIONS_DEBUG) $(ASM_COMPILE_OPTIONS_LISTFILE) -o $@ $<

.PHONY:	clean

# Cleanup rules
clean:
	rm -rf $(OBJ_DIR)
	rm -rf $(TARGET_DIR)