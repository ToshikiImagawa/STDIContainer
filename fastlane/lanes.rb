# coding: utf-8
import File.join(__dir__, '../ResolverGenerator/fastlane/lanes.rb')
import File.join(__dir__, '../Unity/fastlane/lanes.rb')

###
# PARAMS
###

GENERATOR_PARAMS = {
    dotnetPath: "dotnet",
    inputPath:  nil,
    outPath:    nil,
}

GENERATOR_BUILD_PARAMS = {
    buildType:      "Release",
    unityAppPath:   "/Applications/Unity/Unity.app",
}

###
# LANES
###

def lane_release_stdic(lane_context)
    generator_home = generator_get_home()
    home = get_home()
    out_home = File.join(home, "output")
    dotnet = lane_context[:dotnetPath]
    build_type = lane_context[:buildType]
    generator_publish(generator_home, dotnet, build_type)
    lane_release_unity(lane_context)
    FileUtils.rm_rf(out_home)
    FileUtils.mkdir_p(out_home)
    source_path = File.join(generator_home, "output/osx-x64")
    Dir[source_path].each do |source_file|
      FileUtils.cp_r(source_file, out_home)
    end
    source_path = File.join(generator_home, "output/linux-x64")
    Dir[source_path].each do |source_file|
      FileUtils.cp_r(source_file, out_home)
    end
    source_path = File.join(generator_home, "output/win-x64")
    Dir[source_path].each do |source_file|
      FileUtils.cp_r(source_file, out_home)
    end
    source_path = File.join(home, "Unity/output")
    Dir[source_path].each do |source_file|
      if source_file == source_path then
        FileUtils.cp_r(source_file, File.join(out_home, "unity"))
      end
    end
end

###
# PRIVATE METHODS
###

def get_home()
  File.expand_path(File.join(__dir__, ".."))
end

def set_parameters(lane_context, options, hash)
  # default parameters
  hash.each do |key,value|
    lane_context[key] = value
  end

  # parameters from enviroment variables
  hash.each_key do |key|
    env_key = key.to_s # convert symbol to string
    next unless ENV.has_key?(env_key)
    case ENV[env_key]
    when 'true'
      lane_context[key] = true
    when 'false'
      lane_context[key] = false
    else
      lane_context[key] = ENV[env_key]
    end
  end

  # parameters from command arguments
  hash.each_key do |key|
    lane_context[key] = options[key] if options.has_key?(key)
  end
end

