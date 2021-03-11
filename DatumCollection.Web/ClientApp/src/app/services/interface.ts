
/*
 * 定义服务接口
 */ 
export interface IService {
  /*
   * form标题
   */ 
  getFormTitle(): string;

   /*
   * form控件
   */
  getFormControls();

}
