#pragma once

//�����������ݷ�����Ϸ�����Ա����

#include "..\Components\ComponentsCommon.h"
#include "TransformComponent.h"

namespace primal::game_entity {

	DEFINE_TYPED_ID(entity_id);

	class entity {
		public:
			constexpr explicit entity(entity_id id) : _id{ id } {}
			constexpr entity() : _id{ id::invalid_id } {}
			constexpr entity_id get_id() const { return _id; }
			constexpr bool is_valid() const { return id::is_valid(_id); }

			transform::component transform() const;
		private:
			entity_id _id;

	};
}