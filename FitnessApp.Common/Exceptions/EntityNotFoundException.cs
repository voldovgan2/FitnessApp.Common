using System;

namespace FitnessApp.Common.Exceptions;
public class EntityNotFoundException(string entity, string id) : Exception($"Entity {entity} with id: {id} not exist");
