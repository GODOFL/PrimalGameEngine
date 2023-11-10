#pragma once
#include "ComponentsCommon.h"

namespace primal::transform {
	
	struct init_info {
		//����λ��
		f32 position[3]{};
		//��ת��Ԫ��
		f32 rotation[4]{};
		//��������
		f32 scale[3]{ 1.f, 1.f, 1.f };
	};

	component create_transform(const init_info& info, game_entity::entity entity);
	void remove_transform(component c);
}