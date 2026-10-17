import { ApiValidationError } from './api-validation.error';

export interface ProblemDetails {
  type: string;
  title: string;
  status: number;
  detail?: string;
  errors?: ApiValidationError[];
  traceId: string;
}
