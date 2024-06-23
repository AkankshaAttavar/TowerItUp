impl SkillIntrospect<> of dojo::database::introspect::Introspect<Skill<>> {
    #[inline(always)]
    fn size() -> Option<usize> {
        Option::Some(3)
    }

    fn layout() -> dojo::database::introspect::Layout {
        dojo::database::introspect::Layout::Struct(
            array![
                dojo::database::introspect::FieldLayout {
                    selector: 1275106329409434172824427927513550837566786971953511485343475677766844231503,
                    layout: dojo::database::introspect::Introspect::<u16>::layout()
                },
                dojo::database::introspect::FieldLayout {
                    selector: 1699729214882830031587185027751772504613866930833696932445300493213092067149,
                    layout: dojo::database::introspect::Introspect::<u16>::layout()
                },
                dojo::database::introspect::FieldLayout {
                    selector: 1440498530036420181344202512659071066193541043595987749232927146674342635495,
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
                name: 'Skill',
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
                        name: 'attack',
                        attrs: array![].span(),
                        ty: dojo::database::introspect::Introspect::<u16>::ty()
                    },
                    dojo::database::introspect::Member {
                        name: 'strongAttack',
                        attrs: array![].span(),
                        ty: dojo::database::introspect::Introspect::<u16>::ty()
                    },
                    dojo::database::introspect::Member {
                        name: 'healing',
                        attrs: array![].span(),
                        ty: dojo::database::introspect::Introspect::<u16>::ty()
                    }
                ]
                    .span()
            }
        )
    }
}

impl SkillModel of dojo::model::Model<Skill> {
    fn entity(
        world: dojo::world::IWorldDispatcher,
        keys: Span<felt252>,
        layout: dojo::database::introspect::Layout
    ) -> Skill {
        let values = dojo::world::IWorldDispatcherTrait::entity(
            world,
            1170165492864335923838966316993496080799860876794380213775279472483407354402,
            keys,
            layout
        );

        // TODO: Generate method to deserialize from keys / values directly to avoid
        // serializing to intermediate array.
        let mut serialized = core::array::ArrayTrait::new();
        core::array::serialize_array_helper(keys, ref serialized);
        core::array::serialize_array_helper(values, ref serialized);
        let mut serialized = core::array::ArrayTrait::span(@serialized);

        let entity = core::serde::Serde::<Skill>::deserialize(ref serialized);

        if core::option::OptionTrait::<Skill>::is_none(@entity) {
            panic!(
                "Model `Skill`: deserialization failed. Ensure the length of the keys tuple is matching the number of #[key] fields in the model struct."
            );
        }

        core::option::OptionTrait::<Skill>::unwrap(entity)
    }

    #[inline(always)]
    fn name() -> ByteArray {
        "Skill"
    }

    #[inline(always)]
    fn version() -> u8 {
        1
    }

    #[inline(always)]
    fn selector() -> felt252 {
        1170165492864335923838966316993496080799860876794380213775279472483407354402
    }

    #[inline(always)]
    fn instance_selector(self: @Skill) -> felt252 {
        Self::selector()
    }

    #[inline(always)]
    fn keys(self: @Skill) -> Span<felt252> {
        let mut serialized = core::array::ArrayTrait::new();
        core::serde::Serde::serialize(self.entityId, ref serialized);
        core::serde::Serde::serialize(self.gameId, ref serialized);
        core::array::ArrayTrait::span(@serialized)
    }

    #[inline(always)]
    fn values(self: @Skill) -> Span<felt252> {
        let mut serialized = core::array::ArrayTrait::new();
        core::serde::Serde::serialize(self.attack, ref serialized);
        core::serde::Serde::serialize(self.strongAttack, ref serialized);
        core::serde::Serde::serialize(self.healing, ref serialized);
        core::array::ArrayTrait::span(@serialized)
    }

    #[inline(always)]
    fn layout() -> dojo::database::introspect::Layout {
        dojo::database::introspect::Introspect::<Skill>::layout()
    }

    #[inline(always)]
    fn instance_layout(self: @Skill) -> dojo::database::introspect::Layout {
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
trait Iskill<T> {
    fn ensure_abi(self: @T, model: Skill);
}

#[starknet::contract]
mod skill {
    use super::Skill;
    use super::Iskill;

    #[storage]
    struct Storage {}

    #[abi(embed_v0)]
    impl DojoModelImpl of dojo::model::IModel<ContractState> {
        fn selector(self: @ContractState) -> felt252 {
            dojo::model::Model::<Skill>::selector()
        }

        fn name(self: @ContractState) -> ByteArray {
            dojo::model::Model::<Skill>::name()
        }

        fn version(self: @ContractState) -> u8 {
            dojo::model::Model::<Skill>::version()
        }

        fn unpacked_size(self: @ContractState) -> Option<usize> {
            dojo::database::introspect::Introspect::<Skill>::size()
        }

        fn packed_size(self: @ContractState) -> Option<usize> {
            dojo::model::Model::<Skill>::packed_size()
        }

        fn layout(self: @ContractState) -> dojo::database::introspect::Layout {
            dojo::model::Model::<Skill>::layout()
        }

        fn schema(self: @ContractState) -> dojo::database::introspect::Ty {
            dojo::database::introspect::Introspect::<Skill>::ty()
        }
    }

    #[abi(embed_v0)]
    impl skillImpl of Iskill<ContractState> {
        fn ensure_abi(self: @ContractState, model: Skill) {}
    }
}
