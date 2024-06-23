impl HealthIntrospect<> of dojo::database::introspect::Introspect<Health<>> {
    #[inline(always)]
    fn size() -> Option<usize> {
        Option::Some(1)
    }

    fn layout() -> dojo::database::introspect::Layout {
        dojo::database::introspect::Layout::Struct(
            array![
                dojo::database::introspect::FieldLayout {
                    selector: 10081705603222233110711583073880238465441675299724638350815334665239107538,
                    layout: dojo::database::introspect::Introspect::<u16>::layout()
                }
            ]
                .span()
        )
    }

    #[inline(always)]
    fn ty() -> dojo::database::introspect::Ty {
        dojo::database::introspect::Ty::Struct(
            dojo::database::introspect::Struct {
                name: 'Health',
                attrs: array![].span(),
                children: array![
                    dojo::database::introspect::Member {
                        name: 'entityId',
                        attrs: array!['key'].span(),
                        ty: dojo::database::introspect::Introspect::<ContractAddress>::ty()
                    },
                    dojo::database::introspect::Member {
                        name: 'gameId',
                        attrs: array!['key'].span(),
                        ty: dojo::database::introspect::Introspect::<u32>::ty()
                    },
                    dojo::database::introspect::Member {
                        name: 'health',
                        attrs: array![].span(),
                        ty: dojo::database::introspect::Introspect::<u16>::ty()
                    }
                ]
                    .span()
            }
        )
    }
}

impl HealthModel of dojo::model::Model<Health> {
    fn entity(
        world: dojo::world::IWorldDispatcher,
        keys: Span<felt252>,
        layout: dojo::database::introspect::Layout
    ) -> Health {
        let values = dojo::world::IWorldDispatcherTrait::entity(
            world,
            697319301510020699610163733561820948495065627125136602225384869467330756451,
            keys,
            layout
        );

        // TODO: Generate method to deserialize from keys / values directly to avoid
        // serializing to intermediate array.
        let mut serialized = core::array::ArrayTrait::new();
        core::array::serialize_array_helper(keys, ref serialized);
        core::array::serialize_array_helper(values, ref serialized);
        let mut serialized = core::array::ArrayTrait::span(@serialized);

        let entity = core::serde::Serde::<Health>::deserialize(ref serialized);

        if core::option::OptionTrait::<Health>::is_none(@entity) {
            panic!(
                "Model `Health`: deserialization failed. Ensure the length of the keys tuple is matching the number of #[key] fields in the model struct."
            );
        }

        core::option::OptionTrait::<Health>::unwrap(entity)
    }

    #[inline(always)]
    fn name() -> ByteArray {
        "Health"
    }

    #[inline(always)]
    fn version() -> u8 {
        1
    }

    #[inline(always)]
    fn selector() -> felt252 {
        697319301510020699610163733561820948495065627125136602225384869467330756451
    }

    #[inline(always)]
    fn instance_selector(self: @Health) -> felt252 {
        Self::selector()
    }

    #[inline(always)]
    fn keys(self: @Health) -> Span<felt252> {
        let mut serialized = core::array::ArrayTrait::new();
        core::serde::Serde::serialize(self.entityId, ref serialized);
        core::serde::Serde::serialize(self.gameId, ref serialized);
        core::array::ArrayTrait::span(@serialized)
    }

    #[inline(always)]
    fn values(self: @Health) -> Span<felt252> {
        let mut serialized = core::array::ArrayTrait::new();
        core::serde::Serde::serialize(self.health, ref serialized);
        core::array::ArrayTrait::span(@serialized)
    }

    #[inline(always)]
    fn layout() -> dojo::database::introspect::Layout {
        dojo::database::introspect::Introspect::<Health>::layout()
    }

    #[inline(always)]
    fn instance_layout(self: @Health) -> dojo::database::introspect::Layout {
        Self::layout()
    }

    #[inline(always)]
    fn packed_size() -> Option<usize> {
        let layout = Self::layout();

        match layout {
            dojo::database::introspect::Layout::Fixed(layout) => {
                let mut span_layout = layout;
                Option::Some(dojo::packing::calculate_packed_size(ref span_layout))
            },
            dojo::database::introspect::Layout::Struct(_) => Option::None,
            dojo::database::introspect::Layout::Array(_) => Option::None,
            dojo::database::introspect::Layout::Tuple(_) => Option::None,
            dojo::database::introspect::Layout::Enum(_) => Option::None,
            dojo::database::introspect::Layout::ByteArray => Option::None,
        }
    }
}

#[starknet::interface]
trait Ihealth<T> {
    fn ensure_abi(self: @T, model: Health);
}

#[starknet::contract]
mod health {
    use super::Health;
    use super::Ihealth;

    #[storage]
    struct Storage {}

    #[abi(embed_v0)]
    impl DojoModelImpl of dojo::model::IModel<ContractState> {
        fn selector(self: @ContractState) -> felt252 {
            dojo::model::Model::<Health>::selector()
        }

        fn name(self: @ContractState) -> ByteArray {
            dojo::model::Model::<Health>::name()
        }

        fn version(self: @ContractState) -> u8 {
            dojo::model::Model::<Health>::version()
        }

        fn unpacked_size(self: @ContractState) -> Option<usize> {
            dojo::database::introspect::Introspect::<Health>::size()
        }

        fn packed_size(self: @ContractState) -> Option<usize> {
            dojo::model::Model::<Health>::packed_size()
        }

        fn layout(self: @ContractState) -> dojo::database::introspect::Layout {
            dojo::model::Model::<Health>::layout()
        }

        fn schema(self: @ContractState) -> dojo::database::introspect::Ty {
            dojo::database::introspect::Introspect::<Health>::ty()
        }
    }

    #[abi(embed_v0)]
    impl healthImpl of Ihealth<ContractState> {
        fn ensure_abi(self: @ContractState, model: Health) {}
    }
}
