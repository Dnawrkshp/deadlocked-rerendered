# Deadlocked Rerenderer Patch

Facilitates Unity <-> Deadlocked interop.

# Building

1. Install docker

2. Start a docker container with the following arguments

> docker run -it --rm -v "$PWD\:/src" -v "$PWD/../rerenderer/Assets/Resources/Patch:/out" ps2dev/ps2dev:v1.2.0

3. Install dependencies in docker container

> apk add make

4. Build libdlsp

> make -C libdlsp install

5. Build patch

> make
