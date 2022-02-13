# coding: utf-8

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
}

###
# LANES
###

def lane_generator_build(lane_context)
    home = generator_get_home()
    dotnet = lane_context[:dotnetPath]
    build_type = lane_context[:buildType]
    generator_build(home, dotnet, build_type)
end

def lane_generator_run(lane_context)
    home = generator_get_home()
    dotnet = lane_context[:dotnetPath]
    input_path = lane_context[:inputPath]
    out_home = lane_context[:outPath]
    generator_run(home, dotnet, input_path, out_home)
end

def lane_generator_test(lane_context)
    home = generator_get_home()
    dotnet = lane_context[:dotnetPath]
    generator_test(home, dotnet)
end

def lane_generator_build_and_test(lane_context)
    home = generator_get_home()
    dotnet = lane_context[:dotnetPath]
    build_type = lane_context[:buildType]
    generator_build(home, dotnet, build_type)
    generator_test(home, dotnet)
end

def lane_generator_build_and_run(lane_context)
    home = generator_get_home()
    dotnet = lane_context[:dotnetPath]
    build_type = lane_context[:buildType]
    input_path = lane_context[:inputPath]
    out_home = lane_context[:outPath]
    generator_build(home, dotnet, build_type)
    generator_run(home, dotnet, input_path, out_home)
end

def lane_generator_publish(lane_context)
    home = generator_get_home()
    dotnet = lane_context[:dotnetPath]
    build_type = lane_context[:buildType]
    input_path = lane_context[:inputPath]
    out_home = lane_context[:outPath]
    generator_publish(home, dotnet, build_type)
end

###
# PRIVATE METHODS
###

def generator_build(home, dotnet, build_type)
    bridge_generator_project = File.join(home, "ResolverGenerator.sln")
    build_bin_home = File.join(home, "src/ResolverGenerator.Generator/bin")
    build_home = File.join(home, "output")
    FileUtils.rm_rf(build_home)

    # build
    generator_restore_nuget(dotnet, bridge_generator_project)
    sh("#{dotnet} build #{bridge_generator_project} /p:Configuration=#{build_type}")
    # deploy
    source_path = File.join(build_bin_home, "#{build_type}/netcoreapp3.1", "*")
    target_path = File.join(build_home, "ResolverGenerator")
    FileUtils.mkdir_p(target_path)
    Dir[source_path].each do |source_file|
      FileUtils.cp_r(source_file, target_path)
    end
end

def generator_test(home, dotnet)
    bridge_generator_project = File.join(home, "ResolverGenerator.sln")
    # test
    generator_restore_nuget(dotnet, bridge_generator_project)
    sh("#{dotnet} test #{bridge_generator_project}")
end

def generator_run(home, dotnet, input_path, out_home)
    stdic_rg = File.join(home, "output/ResolverGenerator/stdic.dll")
    # run
    sh("#{dotnet} #{stdic_rg} -i #{input_path} -o #{out_home}")
end

def generator_get_home()
  File.expand_path(File.join(__dir__, ".."))
end

def generator_restore_nuget(dotnet, bridge_generator_project)
    sh("#{dotnet} restore #{bridge_generator_project}")
end

def generator_publish(home, dotnet, build_type)
    bridge_generator_project = File.join(home, "src/ResolverGenerator.Generator/ResolverGenerator.Generator.csproj")
    build_bin_home = File.join(home, "src/ResolverGenerator.Generator/bin")
    build_home = File.join(home, "output")
    FileUtils.rm_rf(build_home)

    # build
    generator_restore_nuget(dotnet, bridge_generator_project)
    sh("#{dotnet} publish #{bridge_generator_project} -c #{build_type} --self-contained -r win-x64 -o #{build_bin_home}/../../../output/win-x64")
    sh("#{dotnet} publish #{bridge_generator_project} -c #{build_type} --self-contained -r linux-x64 -o #{build_bin_home}/../../../output/linux-x64")
    sh("#{dotnet} publish #{bridge_generator_project} -c #{build_type} --self-contained -r osx-x64 -o #{build_bin_home}/../../../output/osx-x64")
    # deploy
    source_path = File.join(build_bin_home, "#{build_type}/netcoreapp3.1", "*")
    target_path = File.join(build_home, "ResolverGenerator")
    FileUtils.mkdir_p(target_path)
    Dir[source_path].each do |source_file|
      FileUtils.cp_r(source_file, target_path)
    end
end

def generator_set_parameters(lane_context, options, hash)
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