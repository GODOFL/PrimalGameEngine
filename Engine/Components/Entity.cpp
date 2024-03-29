#include "Entity.h"
#include "Transform.h"

namespace primal::game_entity {
	namespace {

		utl::vector<transform::component>       transforms;

		utl::vector<id::generation_type>        generations;
		utl::deque<entity_id>                   free_ids;
		}// anonymous namespace

	entity create_game_entity(const entity_info& info) {
		assert(info.transform);   //All game entities must have a transform component
		if (!info.transform) return entity{};

		entity_id id;

		if (free_ids.size() > id::min_deleted_elements) {
			id = free_ids.front();
			assert(!is_alive(entity{ id }));
			free_ids.pop_front();
			id = entity_id{ id::new_generation(id) };
			++generations[id::index(id)];
		}
		else {
			id = entity_id{ (id::id_type)generations.size() };
			generations.push_back(0);

			//重新分配components
			//不使用resize重新分配，保持较低的内存分配
			transforms.emplace_back();
		}

		const entity new_entity{ id };
		const id::id_type index{ id::index(id) };

		//Create transform component
		//断言失败？原因
		//没有重新编译整个工程项目导致运行了之前的代码
		assert(!transforms[index].is_valid());
		transforms[index] = transform::create_transform(*info.transform, new_entity);
		if (!transforms[index].is_valid()) return {};

		return new_entity;
	}
	void remove_game_entity(entity e) {
		const entity_id id{ e.get_id() };
		const id::id_type index{ id::index(id) };
		assert(is_alive(e));
		if (is_alive(e)) {
			transform::remove_transform(transforms[index]);
			transforms[index] = {};
			free_ids.push_back(id);
		}
	}

	bool is_alive(entity e) {
		//内存泄露
		assert(e.is_valid());
		const entity_id id{ e.get_id() };
		const id::id_type index{ id::index(id) };
		assert(index < generations.size());
		assert(generations[index] == id::generation(id));
		return (generations[index] == id::generation(id) && transforms[index].is_valid());
	}

	transform::component
	entity::transform() const {
		assert(is_alive(*this));
		const id::id_type index{ id::index(_id) };
		return transforms[index];
	}
}