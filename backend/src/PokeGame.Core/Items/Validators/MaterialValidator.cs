﻿using FluentValidation;
using PokeGame.Core.Items.Properties;

namespace PokeGame.Core.Items.Validators;

internal class MaterialValidator : AbstractValidator<IMaterialProperties>;
