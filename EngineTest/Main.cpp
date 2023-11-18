#pragma comment(lib, "engine.lib")
#include "TestEntityComponents.h"

//Æô¶¯²âÊÔºê
#define TEST_ENTITY_COMPONENTS 1

#ifdef TEST_ENTITY_COMPONENTS

#else
#error One of the tests need to be enable
#endif // TEST_ENTITY_COMPONENTS

int main() {
//¼ì²âÄÚ´æÐ¹Â¶
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	engine_test test{};

	if (test.initialize()) {
		test.run();
	}

	test.shutdown();
}