#pragma comment(lib, "engine.lib")
#include "TestEntityComponents.h"

//�������Ժ�
#define TEST_ENTITY_COMPONENTS 1

#ifdef TEST_ENTITY_COMPONENTS

#else
#error One of the tests need to be enable
#endif // TEST_ENTITY_COMPONENTS

int main() {
//����ڴ�й¶
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif

	engine_test test{};

	if (test.initialize()) {
		test.run();
	}

	test.shutdown();
}