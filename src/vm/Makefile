CC = clang
CFLAGS = -I/usr/include/SDL2
OUT = vm
CFILES = $(shell find . -type f -name '*.c')
OBJECTS = $(CFILES:.c=.o)

all: $(OUT)

run:
	./vm ../../test/test.bin

$(OUT): $(OBJECTS)
	$(CC) $(OBJECTS) $(CFLAGS) -lSDL2 -o $(OUT)
    
%.o: %.c
	$(CC) $(CFLAGS) -c $< -o $@

clean:
	rm -f $(OBJECTS) $(OUT)