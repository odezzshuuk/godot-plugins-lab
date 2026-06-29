@tool
extends Node

func _ready() -> void:
    var log_path := String(ProjectSettings.get_setting("debug/file_logging/log_path"))
    var log_dir := log_path.get_base_dir()

    if not DirAccess.dir_exists_absolute(log_dir):
        return

    var files := DirAccess.get_files_at(log_dir)
    if files.is_empty():
        return

    files.sort()
    var last_log_file := log_dir.path_join(files[files.size() - 1])

    if not FileAccess.file_exists(last_log_file):
        return

    var last_log_contents := FileAccess.get_file_as_string(last_log_file)
    var crash_begin_idx := last_log_contents.find("Program crashed with signal")

    if crash_begin_idx != -1:
        print("===== Previous Session Crashed Backtrace =====\n")
        print(last_log_contents.substr(crash_begin_idx))
