using OpenGM.IO;
using System.Collections;

namespace OpenGM.VirtualMachine.BuiltInFunctions
{
    public static class SkeletonFunctions
    {
        [GMLFunction("skeleton_animation_get", GMLFunctionFlags.Stub)]
        public static object skeleton_animation_get(object?[] args) => "";

        [GMLFunction("skeleton_animation_set", GMLFunctionFlags.Stub)]
        public static object? skeleton_animation_set(object?[] args) => null;

        [GMLFunction("skeleton_animation_get_ext", GMLFunctionFlags.Stub)]
        public static object skeleton_animation_get_ext(object?[] args) => "";

        [GMLFunction("skeleton_animation_set_ext", GMLFunctionFlags.Stub)]
        public static object? skeleton_animation_set_ext(object?[] args) => null;

        [GMLFunction("skeleton_animation_get_duration", GMLFunctionFlags.Stub)]
        public static object skeleton_animation_get_duration(object?[] args) => 0.0;

        [GMLFunction("skeleton_animation_get_frames", GMLFunctionFlags.Stub)]
        public static object skeleton_animation_get_frames(object?[] args) => 0.0;

        [GMLFunction("skeleton_animation_is_finished", GMLFunctionFlags.Stub)]
        public static object skeleton_animation_is_finished(object?[] args) => true;

        [GMLFunction("skeleton_animation_is_looping", GMLFunctionFlags.Stub)]
        public static object skeleton_animation_is_looping(object?[] args) => false;

        [GMLFunction("skeleton_animation_get_event_frames", GMLFunctionFlags.Stub)]
        public static object skeleton_animation_get_event_frames(object?[] args) => new List<object?>();

        [GMLFunction("skeleton_attachment_get", GMLFunctionFlags.Stub)]
        public static object skeleton_attachment_get(object?[] args) => "";

        [GMLFunction("skeleton_attachment_set", GMLFunctionFlags.Stub)]
        public static object skeleton_attachment_set(object?[] args) => true;

        [GMLFunction("skeleton_attachment_create", GMLFunctionFlags.Stub)]
        public static object skeleton_attachment_create(object?[] args) => 0.0;

        [GMLFunction("skeleton_attachment_create_colour", GMLFunctionFlags.Stub)]
        [GMLFunction("skeleton_attachment_create_color", GMLFunctionFlags.Stub)]
        public static object skeleton_attachment_create_colour(object?[] args) => 0.0;

        [GMLFunction("skeleton_attachment_replace", GMLFunctionFlags.Stub)]
        public static object skeleton_attachment_replace(object?[] args) => 0.0;

        [GMLFunction("skeleton_attachment_replace_colour", GMLFunctionFlags.Stub)]
        [GMLFunction("skeleton_attachment_replace_color", GMLFunctionFlags.Stub)]
        public static object skeleton_attachment_replace_colour(object?[] args) => 0.0;

        [GMLFunction("skeleton_bone_data_get", GMLFunctionFlags.Stub)]
        public static object skeleton_bone_data_get(object?[] args) => false;

        [GMLFunction("skeleton_bone_data_set", GMLFunctionFlags.Stub)]
        public static object skeleton_bone_data_set(object?[] args) => false;

        [GMLFunction("skeleton_bone_state_get", GMLFunctionFlags.Stub)]
        public static object skeleton_bone_state_get(object?[] args) => false;

        [GMLFunction("skeleton_bone_state_set", GMLFunctionFlags.Stub)]
        public static object skeleton_bone_state_set(object?[] args) => false;

        [GMLFunction("skeleton_bone_list", GMLFunctionFlags.Stub)]
        public static object? skeleton_bone_list(object?[] args) => null;

        [GMLFunction("skeleton_skin_get", GMLFunctionFlags.Stub)]
        public static object skeleton_skin_get(object?[] args) => "";

        [GMLFunction("skeleton_skin_set", GMLFunctionFlags.Stub)]
        public static object? skeleton_skin_set(object?[] args) => null;

        [GMLFunction("skeleton_skin_list", GMLFunctionFlags.Stub)]
        public static object? skeleton_skin_list(object?[] args) => null;

        [GMLFunction("skeleton_slot_data", GMLFunctionFlags.Stub)]
        public static object? skeleton_slot_data(object?[] args) => null;

        [GMLFunction("skeleton_slot_data_instance", GMLFunctionFlags.Stub)]
        public static object? skeleton_slot_data_instance(object?[] args) => null;

        [GMLFunction("skeleton_slot_list", GMLFunctionFlags.Stub)]
        public static object? skeleton_slot_list(object?[] args) => null;

        [GMLFunction("skeleton_collision_draw_set", GMLFunctionFlags.Stub)]
        public static object? skeleton_collision_draw_set(object?[] args) => null;

        [GMLFunction("skeleton_get_minmax", GMLFunctionFlags.Stub)]
        public static object skeleton_get_minmax(object?[] args) => new List<object?> { 0.0, 0.0, 0.0, 0.0 };

        [GMLFunction("skeleton_get_num_bounds", GMLFunctionFlags.Stub)]
        public static object skeleton_get_num_bounds(object?[] args) => 0.0;

        [GMLFunction("skeleton_get_bounds", GMLFunctionFlags.Stub)]
        public static object skeleton_get_bounds(object?[] args) => new List<object?>();

        [GMLFunction("draw_skeleton_time", GMLFunctionFlags.Stub)]
        public static object? draw_skeleton_time(object?[] args) => null;
    }
}
