using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Registers controllers as well as custom types to Json converters
builder.Services.AddControllers();

// Register Authorization / Authentication services
builder.Services.AddTransient<IAuthorizationPolicyProvider, AuthorizationProvider>();
builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandler>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = Jwt.ValidationParams;
        options.IncludeErrorDetails = true;
    });

// Setup Api Versioning
builder.Services.AddApiVersioning(options =>
{
    // reporting api versions will return the headers "api-supported-versions"
    // options.ReportApiVersions = true;
    
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);

    //This sets the default api version handling
    // options.ApiVersionReader = new UrlSegmentApiVersionReader();

    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("v")
    
        // new QueryStringApiVersionReader("v"),
        // new HeaderApiVersionReader("api-version"),
        // new HeaderApiVersionReader("v")
        // new MediaTypeApiVersionReader("v") //defaults to "v"
        // new MediaTypeApiVersionReader("api-version")
    );
});
builder.Services.AddVersionedApiExplorer(options =>
{
    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
    // note: the specified format code will format the version as "'v'major[.minor][-status]"
    options.GroupNameFormat = "'v'VVV";
    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
    // can also be used to control the format of the API version in route templates
    options.SubstituteApiVersionInUrl = true;
});

// Register Open API documentation support with Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<SwaggerDefaultValues>();
    
    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.\
Enter 'Bearer' [space] and then your token in the text input below.\
Example:


        'Bearer eyJhbGciOiJIUzgsdsxr4dfgdsdfdjkf...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();



var app = builder.Build();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());


app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI(options =>
{

    options.DisplayRequestDuration();
    options.EnablePersistAuthorization();

    // Retrieve Api Versioning Descriptor service registered previously.
    var descriptionProvider = app.Services.GetService<IApiVersionDescriptionProvider>();

    if (descriptionProvider?.ApiVersionDescriptions is null) return;
    
    foreach (var description in descriptionProvider.ApiVersionDescriptions)
    {
        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"v{description.ApiVersion}");
    }
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
