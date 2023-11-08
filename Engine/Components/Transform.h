#pragma once
#include "ComponentsCommon.h"

namespace primal::transform {
	
	DEFINE_TYPED_ID(transform_id);

	struct init_info {
		//物体位置
		f32 position[3]{};
		//旋转四元数
		f32 rotation[4]{};
		//物体缩放
		f32 scale[3]{ 1.f, 1.f, 1.f };
	};

	transform_id create_transform(const init_info& info, game_entity::entity_id entity_id);
	void remove_transform(transform_id id);
}